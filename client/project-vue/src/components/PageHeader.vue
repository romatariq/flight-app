<script setup lang="ts">
import IdentityHeader from './identity/IdentityHeader.vue';
</script>

<template>
    <header>
        <nav className="navbar navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3">
            <div className="container">
                <RouterLink :to="`/`" class="navbar-brand">FlightApp</RouterLink>
                <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                    aria-expanded="false" aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul className="navbar-nav flex-grow-1">                        
                        <li class="nav-item">
                            <RouterLink :to="'/reviews'" class="nav-link text-dark">Reviews</RouterLink>
                        </li>
                    </ul>
                    <ul className="navbar-nav">
                        <IdentityHeader />
                    </ul>
                </div>
            </div>
        </nav>
    </header>
</template>