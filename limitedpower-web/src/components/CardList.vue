<template>
  <div class="hello">
    <div class="container">
      <div v-for="(bundle, index) in this.cardJson" :key="index" class="row">
        <!-- <img :key="card.ArenaId" :alt="card.Name" :src="require('./../assets/img/set/stx/76396-0.jpg' + card.Description)"> -->
        <!-- <span>{{card.ArenaId}}</span> -->
        <div
          v-for="card in bundle"
          :key="card.ArenaId"
          class="column column-20"
        >
          <img :src="card.Path" />
          <small>{{ card.Name }}</small>
          <p>A</p>
        </div>
        <!-- <img :src="card.Path" /> -->
        <!-- <img :src="'@/assets/anatomy.jpg'"> -->
      </div>
    </div>
    <h1>{{ setCode }}</h1>
    <!-- <img v-bind:key="card.ArenaId" v-for="image in cardJson" v-bind:src="image.ArenaId" :alt="image.Name" /> -->
  </div>
</template>

<script>
//const omg = '@/assets/ratings/items.json';
//import(omg).then(res=>window.console.log(res));
//window.console.log("i am top level");
//window.console.log(json);
//  json.forEach(x => { window.console.log(x); });

export default {
  name: "CardList",
  props: {
    setCode: String,
  },
  watch: {
    // whenever question changes, this function will run
    $route(to, from) {
      window.console.log(to);
      window.console.log(from);
      this.Load();
    },
  },
  methods: {
    Load: function () {
      window.console.log("i am created");
      var cardRatings = require("@/assets/ratings/" + this.setCode + ".json");
      cardRatings.forEach(
        (i) =>
          (i.Path = require(`@/assets/img/set/${i.SetCode}/${i.ArenaId}-0.jpg`))
      );

      var cards = [];
      var pkg = [];
      for (var i = 0; i < cardRatings.length; i++) {
        pkg.push(cardRatings[i]);
        if (pkg.length >= 5) {
          cards.push(pkg);
          pkg = [];
        }
      }
      if (pkg.length > 0) {
        cards.push(pkg);
      }
      this.cardJson = cards;
      window.console.log(this.cardJson);
    },
  },
  data: function () {
    return {
      cardJson: [],
      items: [],
      imgs: {},
    };
  },
  created: function () {
    this.Load();
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
