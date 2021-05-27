<template>
  <div class="home">
    <CardList :msg="$route.params.setcode"/>
  </div>
</template>

<script>
// @ is an alias to /src
import CardList from '@/components/CardList.vue'

export default {
  name: 'TopCommons',
  components: {
    CardList
  }
}
</script>
