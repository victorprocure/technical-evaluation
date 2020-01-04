class RoverGrid {
    margin = { "top": 25, "right": 25, "bottom": 25, "left": 25 };
    roverSize = 25;

    constructor(navigationContext, gridElement) {
        this.navigationContext = navigationContext;
        this.gridElement = gridElement;
        this.svgGridRect = this.gridElement.getBoundingClientRect();
        this.gridWidth = this.svgGridRect.width - this.margin.left - this.margin.right;
        this.gridHeight = this.svgGridRect.height - this.margin.top - this.margin.left;

        this.yAxisScale = d3.scale.linear()
            .domain([0, this.navigationContext.plateau.upperRightCoords.y])
            .range([this.gridHeight, 0]);

        this.xAxisScale = d3.scale.linear()
            .domain([0, this.navigationContext.plateau.upperRightCoords.x])
            .range([0, this.gridWidth]);
    }

    get xAxis() {
        return d3.svg.axis()
            .scale(this.xAxisScale)
            .orient("bottom")
            .ticks(this.navigationContext.plateau.upperRightCoords.x);
    }

    get yAxis() {
        return d3.svg.axis()
            .scale(this.yAxisScale)
            .orient("left")
            .ticks(this.navigationContext.plateau.upperRightCoords.y);
    }

    drawMarsExploration() {
        const svgViewport = this.buildSvgViewport();
        const plateau = this.buildPlateau(svgViewport);

        this.addRovers(plateau);
        this.addPaths(plateau);
    }

    addPaths(plateau) {
        plateau.selectAll(".initialPositions")
            .data(this.navigationContext.rovers).enter().append("text")
            .attr("x", d => this.xAxisScale(d.initialCoordinates.x) - 4)
            .attr("y", d => this.yAxisScale(d.initialCoordinates.y) + 6)
            .style("stroke", d => this.createRandomColor())
            .text("X");

        var lineCoords = this.calculateCartesianCoordinatesFromActions();
        plateau.selectAll(".path-lines")
            .data(lineCoords).enter()
            .append("line")
            .attr("x1", d => this.xAxisScale(d.x1))
            .attr("y1", d => this.yAxisScale(d.y1))
            .attr("x2", d => this.xAxisScale(d.x2))
            .attr("y2", d => this.yAxisScale(d.y2))
            .style("stroke", d => d.color)
            .style("opacity", 0.7);
    }

    calculateCartesianCoordinatesFromActions() {
        return this.navigationContext.rovers.map((rover, index) => {
            const navigationActions = this.navigationContext.navigationActions[index];
            let roverPosition = rover.initialCoordinates;
            let roverHeading = rover.initialHeading;

            let previousCoords = { "x": roverPosition.x, "y": roverPosition.y }
            let navigationCoords = [];
            const color = this.createRandomColor();
            navigationActions.forEach(action => {
                if (action == 0) {
                    roverHeading = roverHeading - 1;
                    if (roverHeading < 0)
                        roverHeading = 3;
                }
                if (action == 1) {
                    roverHeading = roverHeading + 1;
                    if (roverHeading > 3)
                        roverHeading = 0;
                }

                if (action == 2) {
                    previousCoords = roverPosition;
                    if (roverHeading == 1)
                        roverPosition = { "x": roverPosition.x + 1, "y": roverPosition.y };

                    if (roverHeading == 3)
                        roverPosition = { "x": roverPosition.x - 1, "y": roverPosition.y };

                    if (roverHeading == 0)
                        roverPosition = { "x": roverPosition.x, "y": roverPosition.y + 1 };

                    if (roverHeading == 2)
                        roverPosition = { "x": roverPosition.x, "y": roverPosition.y - 1 };

                    navigationCoords.push({ "x1": previousCoords.x, "y1": previousCoords.y, "x2": roverPosition.x, "y2": roverPosition.y, color });
                }
            });

            return navigationCoords;
        }).flat(2);
    }

    createRandomColor() {
        const hue = Math.round(Math.random() * 360);
        return `hsl(${hue}, 80%, 65%)`;
    }

    addRovers(plateau) {
        plateau.selectAll(".rovers")
            .data(this.navigationContext.rovers).enter().append("use")
            .attr("xlink:href", d => "#rover")
            .attr("height", this.roverSize)
            .attr("width", this.roverSize)
            .attr("x", d => this.xAxisScale(d.currentCoordinates.x) - (this.roverSize / 2))
            .attr("y", d => this.yAxisScale(d.currentCoordinates.y) - (this.roverSize / 2))
            .attr("transform", d => "rotate(" + this.roverHeadingRotation(d) + ")");
    }

    roverHeadingRotation(rover) {
        const x = rover.currentCoordinates.x,
            y = rover.currentCoordinates.y,
            heading = rover.currentHeading;

        if (heading == 0 || heading > 3)
            return "0";

        var quarter = heading * 0.25;
        var degrees = 360 * quarter;

        return degrees + "," + this.xAxisScale(x) + "," + this.yAxisScale(y);
    }

    buildSvgViewport() {
        const svgViewport = d3.select(this.gridElement)
            .attr("width", this.svgGridRect.width)
            .attr("height", this.svgGridRect.height)
            .style("border", "2px solid");

        return svgViewport;
    }

    buildPlateau(svgViewport) {
        const plateau = svgViewport.append("g")
            .attr("class", "plateau")
            .attr("transform", "translate(" + this.margin.left + "," + this.margin.top + ")");
        plateau.select(".x.axis").call(this.xAxis);
        plateau.select(".y.axis").call(this.yAxis);

        this.stylePlateau(plateau);

        return plateau;
    }

    stylePlateau(plateau) {
        const plottedXAxis = plateau.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + this.gridHeight + ")")
            .call(this.xAxis.tickSize(-this.gridWidth, 0, 0));

        const plottedYAxis = plateau.append("g")
            .attr("class", "y axis")
            .call(this.yAxis.tickSize(-this.gridHeight, 0, 0));

        plottedXAxis.selectAll(".tick line")
            .attr("y1", -this.gridHeight)
            .attr("y2", 0);
        plottedYAxis.selectAll(".tick line")
            .attr("x1", this.gridWidth)
            .attr("x2", 0);
    }
}
