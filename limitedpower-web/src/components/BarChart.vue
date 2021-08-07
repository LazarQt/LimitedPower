<template>
  <div class="container text-center">
    <button v-on:click="ToggleLive()">
        Showing: {{ this.showLiveData ? "Live Ratings" : "Initial Ratings" }}
      </button>
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
  props: {
  },
  created: function () {
    this.Load(true);
  },
  methods: {
        ToggleLive: function () {
      this.showLiveData = !this.showLiveData;
      this.Load(this.showLiveData);
    },
    Load: function (isLive) {
      console.log("loool" + isLive);


        
          fetch("http://localhost:53517/ColorRankings/" +this.$route.params.setcode +"?live=" +isLive.toString()).then(function(response) {
      return response.json()
    }).then(function(response) {
                console.log(response);
          this.series = [{
            data: [response.w,response.u,response.b,response.r,response.g]
          }];
    }.bind(this));



    },
  },
  data: function () {
    return {
      showLiveData: true,
      chartOptions: {
        chart: {
          id: "basic-bar",
        },
        xaxis: {
          categories: ["White", "Blue", "Black", "Red", "Green"],
        },
        colors:["#9b4dca"]
      },
      series: [
        {
          name: 'power level',
          data: [],
        },
      ],
    };
  },
};
</script>