<template>
  <div class="home">
    <CardList :apiCall="'TopCommons'" :setCode="$route.params.setcode" :callParams="'xd'"/>
  </div>
</template>

<script>
import CardList from "@/components/CardList.vue";

export default {
  name: "TierList",
  data: function () {
    return {
      count: 0,
    };
  },
  components: {
    CardList,
  },
};
</script>
