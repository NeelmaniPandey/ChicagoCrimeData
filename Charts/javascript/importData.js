var Data = [
    {
        "YEAR": "2001",
        "OVER $500": "32874",
        "$500 AND UNDER": "61780"
    },
    {
        "YEAR": "2002",
        "OVER $500": "27844",
        "$500 AND UNDER": "47491"
    },
    {
        "YEAR": "2003",
        "OVER $500": "20653",
        "$500 AND UNDER": "37023"
    },
    {
        "YEAR": "2004",
        "OVER $500": "19290",
        "$500 AND UNDER": "35871"
    },
    {
        "YEAR": "2005",
        "OVER $500": "18848",
        "$500 AND UNDER": "27919"
    },
    {
        "YEAR": "2006",
        "OVER $500": "20723",
        "$500 AND UNDER": "27180"
    },
    {
        "YEAR": "2007",
        "OVER $500": "22117",
        "$500 AND UNDER": "25238"
    },
    {
        "YEAR": "2008",
        "OVER $500": "24141",
        "$500 AND UNDER": "25395"
    },
    {
        "YEAR": "2009",
        "OVER $500": "21502",
        "$500 AND UNDER": "24216"
    },
    {
        "YEAR": "2010",
        "OVER $500": "21849",
        "$500 AND UNDER": "24628"
    },
    {
        "YEAR": "2011",
        "OVER $500": "15668",
        "$500 AND UNDER": "28993"
    },
    {
        "YEAR": "2012",
        "OVER $500": "15971",
        "$500 AND UNDER": "29526"
    },
    {
        "YEAR": "2013",
        "OVER $500": "15602",
        "$500 AND UNDER": "27897"
    },
    {
        "YEAR": "2014",
        "OVER $500": "14901",
        "$500 AND UNDER": "28809"
    },
    {
        "YEAR": "2015",
        "OVER $500": "3640",
        "$500 AND UNDER": "7490"
    },
    {
        "YEAR": "2016",
        "OVER $500": "0",
        "$500 AND UNDER": "0"
    }]

var xData = ["OVER $500", "$500 AND UNDER"];

var margin = { top: 20, right: 50, bottom: 30, left: 0 },
    width = 350 - margin.left - margin.right,
    height = 300 - margin.top - margin.bottom;

var x = d3.scale.ordinal()
    .rangeRoundBands([0, width], .35);

var y = d3.scale.linear()
    .rangeRound([height, 0]);

var color = d3.scale.category20();

var xAxis = d3.svg.axis()
    .scale(x)
    .orient("bottom");

var svg = d3.select("#chart").append("svg")
    .attr("width", width + margin.left + margin.right)
    .attr("height", height + margin.top + margin.bottom)
    .append("g")
    .attr("transform", "translate(" + margin.left + "," + margin.top + ")");


var dataIntermediate = xData.map(function (c) {
    return data.map(function (d) {
        return { x: d.YEAR, y: d[c] };
    });
});

var dataStackLayout = d3.layout.stack()(dataIntermediate);

x.domain(dataStackLayout[0].map(function (d) {
    return d.x;
}));

y.domain([0,
    d3.max(dataStackLayout[dataStackLayout.length - 1],
        function (d) { return d.y0 + d.y; })
])
    .nice();

var layer = svg.selectAll(".stack")
    .data(dataStackLayout)
    .enter().append("g")
    .attr("class", "stack")
    .style("fill", function (d, i) {
        return color(i);
    });

layer.selectAll("rect")
    .data(function (d) {
        return d;
    })
    .enter().append("rect")
    .attr("x", function (d) {
        return x(d.x);
    })
    .attr("y", function (d) {
        return y(d.y + d.y0);
    })
    .attr("height", function (d) {
        return y(d.y0) - y(d.y + d.y0);
    })
    .attr("width", x.rangeBand());

svg.append("g")
    .attr("class", "axis")
    .attr("transform", "translate(0," + height + ")")
    .call(xAxis);
