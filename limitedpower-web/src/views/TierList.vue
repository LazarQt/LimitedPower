<template>
  <div class="home">
    <button v-on:click="Test">test</button>
    <CardList :setCode="$route.params.setcode"/>

  </div>
</template>

<script>
// @ is an alias to /src
import CardList from '@/components/CardList.vue'
//import json from '@/assets/ratings/stx.json'

/*
const myVar = '@/assets/ratings/stx.json';
import json2 from myVar;

*/

//const data = await import('../data.json')



export default {
  name: 'TierList',
    data: function () {
    return {
      count: 0
    };
  },
    methods: {
    Test : function() {
 
//window.console.log(x);  
      
    },
  },
  components: {
    CardList
  }
}
</script>
