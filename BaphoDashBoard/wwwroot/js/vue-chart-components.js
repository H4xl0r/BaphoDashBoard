Vue.component('doughnut-chart', {
    props: {
        url:null
    },
    template: ` <div><canvas height="280" ref="chart"></canvas></div>`,
   // mixins: [vue_base],
    data: function () {
        return {
            chart: null
        };
    },
    mounted() {
        this.getChartData();
    },

    methods: {
        getChartData: async function () {

            await axios.get(this.url).then(response => {

                if (response.data !== null) {
                    if (response.data.success !== false) {

                        var charData = [];
                        var dataset = {
                            label: "",
                            backgroundColor: ["#0066CC","#FFD700"],
                            hoverBackgroundColor: ["#0052A3","#DAA520"],
                            data: []
                        };

                        dataset.data = response.data.mrObject.dataSet[0].data;
                        charData.push(dataset);
                        //Dibujo la grafica
                        this.chart = new Chart(this.$refs.chart, {
                            type: 'pie',
                            // The data for our dataset
                            data: {
                                labels: response.data.mrObject.labels,
                                datasets: charData
                            },
                            options: {
                               
                                legend: {
                                    display: false
                                    //position: 'right',
                                    //onClick: function (e) {
                                    //    e.stopPropagation();
                                    //}
                                }
                            },
                        });
                    }
                }
            });

        },
    },

});