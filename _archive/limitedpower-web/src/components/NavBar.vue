<template>
  <div id="app">
    <div id="nav">
      <div>
        <router-link to="/">Home</router-link> |
        <router-link :to="'/tierlist/' + GetUrl()">Tier List</router-link> |
        <router-link :to="'/topcommons/' + GetUrl()">Top Commons</router-link> |
        <router-link :to="'/colorrankings/' + GetUrl()"
          >Color Rankings</router-link
        >
        for
        <span v-for="(item, index) in myJson.Sets" :key="item.code">
          <router-link :to="GetSetUrl(item.code)">{{
            item.code.toUpperCase()
          }}</router-link
          ><span v-if="index + 1 < myJson.Sets.length"> | </span>
        </span>
      </div>
    </div>
    <router-view />
  </div>
</template>

<script>
import json from "@/assets/config.json";

export default {
  name: "NavBar",
  data: function () {
    return {
      count: 0,
      myJson: json,
    };
  },
  methods: {
    GetSetUrl: function (setcode) {
      var name = this.$route.name;
      var start = "";
      if (name == null || name == "Home") {
        start = "/tierlist/";
      }
      return start + setcode;
    },
    GetUrl: function () {
      var setCode = this.$route.params.setcode;
      if (setCode == null) return "afr";
      return setCode;
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
