<script setup lang="ts">
const props = defineProps<{
    rating: number,
    setRating?: (rating: number) => void
}>() 


const handleClick = (i: number): void => {
    if (props.setRating) {
        props.setRating(i);
    }
}
</script>

<template>
    <div>
        <span v-for=" i in [1, 2, 3, 4, 5]" :key="i"
            :class="'fa fa-star' + (props.rating >= i ? ' checked' : '') + (props.setRating ? ' user-rating' : '')"
            @click="() => {handleClick(i)}" >
        </span>
    </div>
</template>

<style scoped>
.fa-star.checked {
    color: orange;
}
.fa-star.user-rating{
    cursor: pointer;
}
</style>
