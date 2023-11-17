<script setup lang="ts">
import type { IReviewCategory } from '@/domain/IReviewCategory';
import { ReviewService } from '@/services/ReviewService';
import { ref } from 'vue';

const props = defineProps<{
    setDefaultCategory: boolean,
    selectedCategoryId: string,
    setSelectedCategoryId: (id: string) => void
    setPage?: (page: number) => void
}>()

const reviewService = new ReviewService();

const categories = ref(undefined as IReviewCategory[] | undefined);

const fetchCategories = async () => {
    const fetchedCategories = await reviewService.getCategories();
    if (fetchedCategories) {
        categories.value = fetchedCategories;
        if (categories.value.length > 0 && props.setDefaultCategory) {
            props.setSelectedCategoryId(fetchedCategories[0].id!);
        }
    }
};
fetchCategories();

const handleClick = (id: string) => {
    props.setSelectedCategoryId(id);
    if (props.setPage) {
        props.setPage(1);
    }
}


</script>

<template>
    <div v-if="categories">
        <button
            v-for="category in categories" :key="category.id"
            :class="category.id === props.selectedCategoryId ? 'purple' : ''"
            @click="() => {handleClick(category.id!)}" >
            {{category.category}}
        </button>
    </div>
</template>