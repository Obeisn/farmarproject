var ctx = document.getElementById('RadarChart');
var chart = new Chart(ctx, {
    type: 'radar',
    data: {
        labels: ["新潮", "價格", "品質", "性能", "油耗", "配備"],

        datasets: [{
            label: "SUV",
            data: [90, 70, 80, 88, 50, 65],
            backgroundColor: 'rgba(173,255,47, 0.5)',
            borderColor: 'rgb(0,0,0)',
            pointStyle: 'circle',
            pointBackgroundColor: 'rgb(0,0,255)',
            pointRadius: 5,
            pointHoverRadius: 10,
        },
        {
            label: "轎車",
            data: [64, 82, 85, 76, 93, 58],
            backgroundColor: 'rgba(255,105,180, 0.5)',
            borderColor: 'rgb(0,0,0)',
            pointStyle: 'rect',
            pointBackgroundColor: 'rgb(255,0,0)',
            pointRadius: 5,
            pointHoverRadius: 10,
        }]
    },
    options: {
        responsive: true,
        legend: {
            position: 'top',
            labels: {
                fontColor: 'black',
                fontSize: 24
            }
        },
        scale: {
            ticks: {
                beginAtZero: true
            },
            pointLabels: {
                fontSize: 20
            },
        },
    }
});