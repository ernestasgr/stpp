<script lang="ts">
	import '../app.postcss';
	import { AppShell, AppBar, Avatar, AppRail, AppRailAnchor, TabGroup, TabAnchor } from '@skeletonlabs/skeleton';
	import { accessToken, isAdminStore, usernameStore } from '../stores';
	import { initializeStores, Drawer, getDrawerStore } from '@skeletonlabs/skeleton';
	import '@fortawesome/fontawesome-free/css/all.min.css'

	// Floating UI for Popups
	import { computePosition, autoUpdate, flip, shift, offset, arrow } from '@floating-ui/dom';
	import { storePopup } from '@skeletonlabs/skeleton';
	storePopup.set({ computePosition, autoUpdate, flip, shift, offset, arrow });

	let accessTokenValue: string;
	let username: string;
	let isAdmin: boolean;

	accessToken.subscribe(value => {
		accessTokenValue = value;
	})
	usernameStore.subscribe(value => {
		username = value;
	})
	isAdminStore.subscribe(value => {
		isAdmin = value;
	})

	initializeStores();

	const drawerStore = getDrawerStore();

	function drawerOpen(): void {
		drawerStore.open({});
	}

	function drawerClose(): void {
     	drawerStore.close();
	}
</script>

<Drawer>
	<nav class="list-nav p-4">
		<ul>
			<li><a href="/" on:click={drawerClose}>Home</a></li>
			<li><a href="/register" on:click={drawerClose}>Register</a></li>
			<li><a href="/login" on:click={drawerClose}>Login</a></li>
			{#if isAdmin}
				<li><a href="/create-movie" on:click={drawerClose}>Create Movie</a></li>
				<li><a href="/edit-movie" on:click={drawerClose}>Edit Movie</a></li>
			{/if}
		</ul>
		
	</nav>
</Drawer>

<!-- App Shell -->
<AppShell>	
	<svelte:fragment slot="header">
		<AppBar>
			<svelte:fragment slot="trail">
				<a
					class="btn btn-sm variant-ghost-surface invisible md:visible"
					href="/register"
				>
					Register
				</a>

				<a
					class="btn btn-sm variant-ghost-surface invisible md:visible"
					href="/login"
				>
					Login
				</a>

				{#if accessTokenValue !== ''}
					<div class="invisible md:visible">
						<Avatar initials={username}/>
					</div>
				{/if}
			</svelte:fragment>

			<svelte:fragment slot="lead">
				<div class="flex items-center">
					<button class="md:hidden btn btn-sm mr-4" on:click={drawerOpen}>
						<span>
							<svg viewBox="0 0 100 80" class="fill-token w-4 h-4">
								<rect width="100" height="20" />
								<rect y="30" width="100" height="20" />
								<rect y="60" width="100" height="20" />
							</svg>
						</span>
					</button>
					<a href="/"><strong class="text-xl uppercase"><span style="color:#d4163c">Movie</span> Theatre</strong></a>
					{#if isAdmin}
						<a
						class="btn btn-sm variant-ghost-surface invisible md:visible"
						href="/create-movie"
						>
						Create Movie
						</a>
						<a
							class="btn btn-sm variant-ghost-surface invisible md:visible"
							href="/edit-movie"
						>
						Edit Movie
						</a>
					{/if}
					
				</div>
			</svelte:fragment>

		</AppBar>
	</svelte:fragment>

	<svelte:fragment slot="pageFooter">
		<AppBar>
			<svelte:fragment slot="lead">
				<p class="text-s">
					Website made by Ernestas Grubis
				</p>
			</svelte:fragment>

			<svelte:fragment slot="trail">
				<a
					class="btn btn-sm variant-ghost-surface"
					href="https://github.com/ernestasgr/stpp"
					target="_blank"
					rel="noreferrer"
				>
					Project GitHub
				</a>
			</svelte:fragment>
		</AppBar>
	</svelte:fragment>

	<slot />
</AppShell>
