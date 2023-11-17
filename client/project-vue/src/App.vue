<script setup lang="ts">
import {  RouterView } from 'vue-router'
import PageHeader from './components/PageHeader.vue';
</script>

<template>
    <PageHeader />
    <div className="container">
        <main role="main" className="pb-3">
            <RouterView />
        </main>
    </div>
</template>

<style scoped>
</style>
