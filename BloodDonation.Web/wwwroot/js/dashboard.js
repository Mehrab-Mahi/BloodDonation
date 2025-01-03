﻿BloodDonation.Settings.Logout = function () {
    window.localStorage.clear();
    $.ajax({
        url: '/Account/Logout',
        type: "GET",
        dataType: "json",
        success: function (data) {
            window.location.href = '/Account/Login';
        },
        error: function (e) {
            window.location.href = '/Account/Login';
        }
    });
}

BloodDonation.Settings.ShowDummyData = function () {
    // Sparkline Charts
    $('.inlinebar').sparkline('html', { type: 'bar', barColor: '#ff6264' });
    $('.inlinebar-2').sparkline('html', { type: 'bar', barColor: '#445982' });
    $('.inlinebar-3').sparkline('html', { type: 'bar', barColor: '#00b19d' });
    $('.bar').sparkline([[1, 4], [2, 3], [3, 2], [4, 1]], { type: 'bar' });
    $('.pie').sparkline('html', { type: 'pie', borderWidth: 0, sliceColors: ['#3d4554', '#ee4749', '#00b19d'] });
    $('.linechart').sparkline();
    $('.pageviews').sparkline('html', { type: 'bar', height: '30px', barColor: '#ff6264' });
    $('.uniquevisitors').sparkline('html', { type: 'bar', height: '30px', barColor: '#00b19d' });

    $(".monthly-sales").sparkline([1, 2, 3, 5, 6, 7, 2, 3, 3, 4, 3, 5, 7, 2, 4, 3, 5, 4, 5, 6, 3, 2], {
        type: 'bar',
        barColor: '#485671',
        height: '80px',
        barWidth: 10,
        barSpacing: 2
    });

    // Line Charts
    var line_chart_demo = $("#line-chart-demo");

    var line_chart = Morris.Line({
        element: 'line-chart-demo',
        data: [
            { y: '2006', a: 100, b: 90 },
            { y: '2007', a: 75, b: 65 },
            { y: '2008', a: 50, b: 40 },
            { y: '2009', a: 75, b: 65 },
            { y: '2010', a: 50, b: 40 },
            { y: '2011', a: 75, b: 65 },
            { y: '2012', a: 100, b: 90 }
        ],
        xkey: 'y',
        ykeys: ['a', 'b'],
        labels: ['October 2013', 'November 2013'],
        redraw: true
    });

    line_chart_demo.parent().attr('style', '');

    // Donut Chart
    var donut_chart_demo = $("#donut-chart-demo");

    donut_chart_demo.parent().show();

    var donut_chart = Morris.Donut({
        element: 'donut-chart-demo',
        data: [
            { label: "Download Sales", value: getRandomInt(10, 50) },
            { label: "In-Store Sales", value: getRandomInt(10, 50) },
            { label: "Mail-Order Sales", value: getRandomInt(10, 50) }
        ],
        colors: ['#707f9b', '#455064', '#242d3c']
    });

    donut_chart_demo.parent().attr('style', '');

    // Area Chart
    var area_chart_demo = $("#area-chart-demo");

    area_chart_demo.parent().show();

    var area_chart = Morris.Area({
        element: 'area-chart-demo',
        data: [
            { y: '2006', a: 100, b: 90 },
            { y: '2007', a: 75, b: 65 },
            { y: '2008', a: 50, b: 40 },
            { y: '2009', a: 75, b: 65 },
            { y: '2010', a: 50, b: 40 },
            { y: '2011', a: 75, b: 65 },
            { y: '2012', a: 100, b: 90 }
        ],
        xkey: 'y',
        ykeys: ['a', 'b'],
        labels: ['Series A', 'Series B'],
        lineColors: ['#303641', '#576277']
    });

    area_chart_demo.parent().attr('style', '');

    // Rickshaw
    var seriesData = [[], []];

    var random = new Rickshaw.Fixtures.RandomData(50);

    for (var i = 0; i < 50; i++) {
        random.addData(seriesData);
    }

    var graph = new Rickshaw.Graph({
        element: document.getElementById("rickshaw-chart-demo"),
        height: 193,
        renderer: 'area',
        stroke: false,
        preserve: true,
        series: [{
            color: '#73c8ff',
            data: seriesData[0],
            name: 'Upload'
        }, {
            color: '#e0f2ff',
            data: seriesData[1],
            name: 'Download'
        }
        ]
    });

    graph.render();

    var hoverDetail = new Rickshaw.Graph.HoverDetail({
        graph: graph,
        xFormatter: function (x) {
            return new Date(x * 1000).toString();
        }
    });

    var legend = new Rickshaw.Graph.Legend({
        graph: graph,
        element: document.getElementById('rickshaw-legend')
    });

    var highlighter = new Rickshaw.Graph.Behavior.Series.Highlight({
        graph: graph,
        legend: legend
    });

    setInterval(function () {
        random.removeData(seriesData);
        random.addData(seriesData);
        graph.update();
    }, 500);
}

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

BloodDonation.Settings.LoadUserMenu = function (id) {
    var keyName = 'role_' + id;
    var menu = '<li id="10000"><a href="/"><i class="entypo-gauge"></i><span class="title">Dashboard</span></a></li>';
    var menuList = window.localStorage.getItem(keyName);
    if (!menuList) {
        appClient.get('/users/getusermenu/' + id, null,
            function (data) {
                window.localStorage.setItem(keyName, JSON.stringify(data));
                window.localStorage.setItem("hasMenu", "yes");
                var sortedParents = _.sortBy(data, 'sortOrder');
                for (var i = 0; i < sortedParents.length; i++) {
                    menu = menu + '<li class="has-sub menu" id="' + sortedParents[i].id + '"><a href="#"><i class="' + sortedParents[i].icon + '"></i>';
                    menu = menu + '<span class="title">' + sortedParents[i].name + '</span></a>';
                    var sortedChilds = _.sortBy(sortedParents[i].child, 'sortOrder');
                    for (var j = 0; j < sortedChilds.length; j++) {
                        menu = menu + '<ul id="' + sortedChilds[j].id + '"><li id="' + sortedChilds[j].menuId + '" ><a href="' + sortedChilds[j].url + '">';
                        menu = menu + '<i class="' + sortedChilds[j].icon + '"></i><span class="title">' + sortedChilds[j].name + '</span></a></li></ul>';
                    }
                    menu = menu + '</li >';
                }

                $('#main-menu').html(menu);

                // Sidebar Menu Setup
                setup_sidebar_menu();
            })
    }
    else {
        window.localStorage.setItem("hasMenu", "yes");
        var data = JSON.parse(menuList);

        var sortedParents = _.sortBy(data, 'sortOrder');
        for (var i = 0; i < sortedParents.length; i++) {
            menu = menu + '<li class="has-sub menu" id="' + sortedParents[i].id + '"><a href="#"><i class="' + sortedParents[i].icon + '"></i>';
            menu = menu + '<span class="title">' + sortedParents[i].name + '</span></a>';
            var sortedChilds = _.sortBy(sortedParents[i].child, 'sortOrder');
            for (var j = 0; j < sortedChilds.length; j++) {
                menu = menu + '<ul id="' + sortedChilds[j].id + '"><li id="' + sortedChilds[j].menuId + '" ><a href="' + sortedChilds[j].url + '">';
                menu = menu + '<i class="' + sortedChilds[j].icon + '"></i><span class="title">' + sortedChilds[j].name + '</span></a></li></ul>';
            }
            menu = menu + '</li >';
        }

        $('#main-menu').html(menu);

        // Sidebar Menu Setup
        setup_sidebar_menu();
    }
}