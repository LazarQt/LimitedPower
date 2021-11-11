<template>
  <BarChart />
</template>

<script>
import BarChart from "@/components/BarChart.vue";

export default {
  name: "ColorRankings",
  data: function () {
    return {
      count: 0,
    };
  },
  components: {
    BarChart,
  },
};
</script>
