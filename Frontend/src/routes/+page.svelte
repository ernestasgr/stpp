<script>
	import { onMount } from 'svelte';
	import '@fortawesome/fontawesome-free/css/all.min.css';

	/**
	 * @type {any[]}
	 */
	let movies = [];

	onMount(() => {
		fetch(
			'https://stpp-ernestas-grubis-backend.azurewebsites.net/api/v1/movies?PageNumber=1&PageSize=50',
			{
				method: 'GET',
				headers: {
					'Content-Type': 'application/json'
				}
			}
		)
			.then(async (response) => {
				if (!response.ok) {
					throw await response.text();
				} else {
					return response.json();
				}
			})
			.then((response) => {
				movies = response;
			});
	});
</script>

<div>
	<div class="container h-full mx-auto flex justify-center items-center">
		<div class="space-y-10 text-center flex flex-col items-center">
			<br />
			<strong class="text-xl uppercase"><span style="color:#d4163c">Available</span> Movies</strong>

			<section class="grid grid-cols-2 md:grid-cols-5 gap-4">
				{#each movies as movie}
					<div>
						<a href={'movies/' + movie.id} class="shrink-0 w-[28%] snap-start">
							<img
								class="rounded-container-token hover:brightness-125"
								src={movie.mainImage}
								alt={movie.title}
								title={movie.title}
								loading="lazy"
							/>
						</a>
					</div>
				{/each}
			</section>
		</div>
	</div>
</div>
