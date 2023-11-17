<script setup lang="ts">
import ReviewCategories from '@/components/ReviewCategories.vue';
import ReviewsList from '@/components/ReviewsList.vue';
import { ref } from 'vue';

const selectedCategoryId = ref<string>("");
const page = ref<number>(1);
const airportIata = ref<string>("TLL");
</script>

<template>
    <div class="review-page">
        <h1>TLL</h1>
        <ReviewCategories 
            :setDefaultCategory="true" :selectedCategoryId="selectedCategoryId" 
            :setSelectedCategoryId="(id: string) => selectedCategoryId=id" :setPage="(nr : number) => page=nr" />
        <ReviewsList 
            :airportIata="airportIata" :selectedCategoryId="selectedCategoryId" 
            :setSelectedCategoryId="(id: string) => selectedCategoryId=id" :page="page" :setPage="(nr: number) => page = nr" />
    </div>
</template>