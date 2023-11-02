import { writable } from 'svelte/store';

export const accessToken = writable('');
export const usernameStore = writable('');
export const isAdminStore = writable(false);