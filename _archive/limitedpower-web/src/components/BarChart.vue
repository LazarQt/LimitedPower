<template>
  <div class="container text-center">
    <div style="position: fixed; right: 10px; bottom: 10px">
      <button v-on:click="ToggleLive()" style="border-radius: 44px">
        Showing: {{ this.showLiveData ? "Live Ratings" : "Initial Ratings" }}
      </button>
    </div>
    <apexcharts
      height="350"
      type="bar"
      :options="chartOptions"
      :series="series"
    ></apexcharts>
  </div>
</template>

<script>
import VueApexCharts from "vue-apexcharts";

export default {
  name: "BarChart",
  components: {
    apexcharts: VueApexCharts,
  },
  watch: {
    $route() {
      this.Load(true);
    },
  },
  props: {},
  created: function () {
    this.Load(true);
  },
  methods: {
    ToggleLive: function () {
      this.showLiveData = !this.showLiveData;
      this.Load(this.showLiveData);
    },
    Load: function (isLive) {
      // https://lpconfig.azurewebsites.net/ColorRankings/
      fetch(
        "https://lpconfig.azurewebsites.net/ColorRankings/" +
          this.$route.params.setcode +
          "?live=" +
          isLive.toString()
      )
        .then(function (response) {
          return response.json();
        })
        .then(
          function (response) {
            this.series = [
              {
                data: [
                  response.w,
                  response.u,
                  response.b,
                  response.r,
                  response.g,
                ],
              },
            ];
          }.bind(this)
        );
    },
  },
  data: function () {
    return {
      showLiveData: true,
      chartOptions: {
        chart: {
          type: "bar",
          height: 350,
        },
        plotOptions: {
          bar: {
            colors: {
              ranges: [
                {
                  from: -100,
                  to: -46,
                  color: "#F15B46",
                },
                {
                  from: -45,
                  to: 0,
                  color: "#FEB019",
                },
              ],
            },
            columnWidth: "80%",
          },
        },
        dataLabels: {
          enabled: false,
        },
        yaxis: {
          title: {
            text: "Relative Power Level",
          },
          labels: {
            formatter: function (y) {
              return y.toFixed(0) + "%";
            },
          },
        },
        xaxis: {
          type: "string",
          categories: ["White", "Blue", "Black", "Red", "Green"],
          labels: {
            rotate: -90,
          },
        },
      },
      series: [
        {
          name: "Relative Power Level",
          data: [],
        },
      ],
    };
  },
};
</script>