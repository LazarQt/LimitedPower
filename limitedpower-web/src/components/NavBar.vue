<template>
  <div id="app">
    <div id="nav">
      <!-- <button v-on:click="Do">test</button> -->

      <div>
        <router-link to="/">Home</router-link> |
        <router-link :to="'/tierlist/' + GetSetCode()"
          >Tier List</router-link
        >
        |
        <router-link :to="'/topcommons/' + GetSetCode()"
          >Top Commons</router-link
        >
        for
        <span v-for="item in myJson.Sets" :key="item.code">
          <router-link :to="item.code">{{ item.code }}</router-link> |
        </span>
      </div>

      <!-- <div>
        <router-link to="/">Home</router-link> |
        <router-link to="/about">About</router-link> |
        <router-link to="/tierlist">Tier List</router-link>
      </div>
      <div>
        <router-link to="/about">Strixhaven</router-link> |
        <router-link to="/tierlist">Kaldheim</router-link>
      </div> -->
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
    GetSetCode : function() {
      
      var setCode = this.$route.params.setcode;
      if(setCode == null) return "stx";
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
