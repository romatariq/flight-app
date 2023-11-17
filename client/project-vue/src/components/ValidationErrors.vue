<script setup lang="ts">
defineProps<{
    validationErrors: string[]
}>()
</script>


<template>
    <ul v-if="validationErrors.length > 0">
        <li v-for="error in validationErrors" :key="error">
            {{ error }}
        </li>
    </ul>
</template>

<style scoped>
    ul {
        color: red;
    }
</style>