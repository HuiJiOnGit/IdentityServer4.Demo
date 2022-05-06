<template>
  <el-container>
    <el-header><h1>This is a private page</h1></el-header>
    <el-main>
      <el-row justify="center">
        <el-button id="login" type="primary" @click="login">Login</el-button>
        <el-button id="api" type="success" @click="api">Call API</el-button>
        <el-button id="logout" type="info" @click="logout">Logout</el-button>
      </el-row>
      <el-row justify="center">
        <el-input
          v-model="text"
          :rows="20"
          type="textarea"
          placeholder="Please input"
        />
      </el-row>
    </el-main>
  </el-container>
</template>

<script setup>
import Oidc from "oidc-client";
import { ref } from "vue";
import { ElRow, ElButton, ElMessage } from "element-plus";
const text = ref("");
const apiUrl = "https://localhost:5001/identity";
const config = {
  authority: "https://localhost:5000",
  client_id: "js",
  redirect_uri: "https://localhost:5003/callback",
  response_type: "code",
  scope: "openid profile api1",
  post_logout_redirect_uri: "https://localhost:5003/home",
};
const mgr = new Oidc.UserManager(config);

const login = () => {
  mgr.signinRedirect();
};
const api = () => {
  mgr
    .getUser()
    .then((user) => {
      const xhr = new XMLHttpRequest();
      xhr.open("GET", apiUrl);
      xhr.onload = function () {
        // log(xhr.status, JSON.parse(xhr.responseText));
        ElMessage(xhr.status);
        text.value = xhr.responseText;
      };
      xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
      xhr.send();
    })
    .catch((err) => {
      ElMessage.error(err);
    });
};
const logout = () => {
  mgr.signoutRedirect();
};
</script>

<style scoped>
textarea {
  width: 50vw;
  height: 25vh;
}
</style>
