<script setup lang="ts">
import type { IRegisterData } from '@/dto/IRegisterData';
import router from '@/router';
import { IdentityService } from '@/services/IdentityService';
import { ref } from 'vue';
import ValidationErrors from '@/components/ValidationErrors.vue';

const identityService = new IdentityService();

const values = ref({
    password: "",
    confirmPassword: "",
    email: "",
    firstName: "",
    lastName: "",
} as IRegisterData);

const validationErrors = ref([] as string[]);

const onSubmit = async (event: MouseEvent) => {
    event.preventDefault();

    if (values.value.firstName.length === 0 || values.value.lastName.length === 0 || values.value.email.length === 0 ||
        values.value.password.length === 0 || values.value.confirmPassword.length === 0) {
        validationErrors.value = ["Fields cannot be empty!"];
        return;
    }
    if (values.value.password !== values.value.confirmPassword) {
        validationErrors.value = ["Confirm password has to match password!"];
        return;
    }

    // remove errors
    validationErrors.value = [];

    var jwtData = await identityService.register(values.value);
    if (jwtData === undefined) {
        validationErrors.value = ["no jwt - invalid inputs"];
        return;
    }
    router.push("/login");
}

</script>

<template>
    <form class="form-signin w-100 m-auto">
        <h2>Create a new account.</h2>
        <hr />

        <ValidationErrors :validation-errors="validationErrors" />

        <div class="form-floating mb-3">
            <input v-model="values.email" class="form-control" autoComplete="username" aria-required="true"
                placeholder="name@example.com" type="email" id="Input_Email" name="email" />
            <label htmlFor="Input_Email">Email</label>
        </div>
        <div class="form-floating mb-3">
            <input v-model="values.firstName" class="form-control" autoComplete="firstname" aria-required="true"
                placeholder="FirstName" type="text" id="Input_FirstName" name="firstName" />
            <label htmlFor="Input_FirstName">First name</label>
        </div>
        <div class="form-floating mb-3">
            <input v-model="values.lastName" class="form-control" autoComplete="lastname" aria-required="true"
                placeholder="LastName" type="text" id="Input_LastName" name="lastName" />
            <label htmlFor="Input_LastName">Last name</label>
        </div>
        <div class="form-floating mb-3">
            <input v-model="values.password" class="form-control" autoComplete="new-password" aria-required="true"
                placeholder="password" type="password" id="Input_Password" name="password" />
            <label htmlFor="Input_Password">Password</label>
        </div>
        <div class="form-floating mb-3">
            <input v-model="values.confirmPassword" class="form-control" autoComplete="new-password"
                aria-required="true" placeholder="password" type="password" id="Input_ConfirmPassword"
                name="confirmPassword" />
            <label htmlFor="Input_ConfirmPassword">Confirm Password</label>
        </div>

        <button @click="(e) => onSubmit(e)" id="registerSubmit" class="w-100 btn btn-lg btn-primary">Register
        </button>

    </form>
</template>