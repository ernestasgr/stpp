<script lang="ts">
	import { onMount } from 'svelte';
	import '@fortawesome/fontawesome-free/css/all.min.css';
	import { getModalStore, type ModalComponent, type ModalSettings } from '@skeletonlabs/skeleton';
	import SeatingGrid from '../../../SeatingGrid.svelte';

	export let data;
	/**
	 * @type {HTMLDivElement}
	 */
	let elemCarousel: HTMLDivElement;

	const modalStore = getModalStore();

	const modalComponent: ModalComponent = { ref: SeatingGrid };

	const modal: ModalSettings = {
		type: 'component',
		component: modalComponent,
		meta: {}
	};

	$: movie = {
		id: data.slug,
		title: '',
		description: '',
		releaseDate: '',
		director: '',
		mainImage: '',
		images: [],
		videos: []
	};

	$: showings = [
		{
			startTime: '2000-01-01T08:00:00Z',
			endTime: '2000-01-02T08:00:00Z',
			number: ''
		}
	];

	onMount(() => {
		fetch(`http://localhost:5157/api/v1/movies/${data.slug}`, {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json'
			}
		})
			.then(async (response) => {
				console.log(response);
				if (!response.ok) {
					throw await response.text();
				} else {
					return response.json();
				}
			})
			.then((response) => {
				movie = response;
				console.log(movie);
				return response;
			})
			.then((response) => {
				fetch(
					`http://localhost:5157/api/v1/movies/${data.slug}/showings?PageNumber=1&PageSize=50`,
					{
						method: 'GET',
						headers: {
							'Content-Type': 'application/json'
						}
					}
				).then(async (response) => {
					console.log(response);
					if (!response.ok) {
						throw await response.text();
					} else {
						showings = await response.json();
						return showings;
					}
				});
			})
			.catch((error) => console.error('Error fetching movie', error));
	});

	function carouselLeft() {
		const x =
			elemCarousel.scrollLeft === 0
				? elemCarousel.clientWidth * elemCarousel.childElementCount // loop
				: elemCarousel.scrollLeft - elemCarousel.clientWidth; // step left
		elemCarousel.scroll(x, 0);
	}

	/**
	 * @param {number} index
	 */
	function carouselThumbnail(index: number) {
		elemCarousel.scroll(elemCarousel.clientWidth * index, 0);
	}

	function carouselRight() {
		console.log(elemCarousel.scrollLeft);
		console.log(elemCarousel.scrollWidth - elemCarousel.clientWidth);
		const x =
			Math.ceil(elemCarousel.scrollLeft) === elemCarousel.scrollWidth - elemCarousel.clientWidth
				? 0 // loop
				: elemCarousel.scrollLeft + elemCarousel.clientWidth; // step right
		elemCarousel.scroll(x, 0);
	}

	/**
	 * @param {string} date
	 */
	function dateToUserFriendly(date: string) {
		let splitDate = date.split('T');
		return splitDate[0] + ' ' + splitDate[1].split(':')[0] + ':' + splitDate[1].split(':')[1];
	}

	function openModal(showingId: string) {
		modal.meta.movieId = movie.id;
		modal.meta.showingId = showingId;
		modalStore.trigger(modal);
	}
</script>

<div class="rounded-lg p-4 shadow-md">
	<div class="flex">
		<!-- Image container -->
		<div class="w-1/2 pr-2 h-auto max-h-[75vh] overflow-hidden">
			<img src={movie.mainImage} alt={movie.title} class="w-auto rounded-lg" />
		</div>

		<!-- Video container -->
		<div class="w-1/2 pl-2 h-auto max-h-[75vh] relative">
			<div class="w-full h-0 pb-[56.25%] bg-black">
				<iframe
					class="absolute inset-0 w-full h-full video-player"
					src={movie.videos[0]}
					frameborder="0"
					allowfullscreen
					title="trailer"
				/>
			</div>
		</div>
	</div>

	<h1 class="text-2xl font-semibold mt-2">{movie.title}</h1>
	<p class="text-gray-500">Released in: {movie.releaseDate.split('T')[0]}</p>
	<p class="text-gray-500">Directed by: {movie.director}</p>

	<p class="mt-4">{movie.description}</p>
	<div
		class="flex justify-center items-center mx-auto w-[75%] grid grid-cols-1 gap-4 hidden md:block"
	>
		<div class="card p-4 grid grid-cols-[auto_1fr_auto] gap-4 items-center">
			<!-- Button: Left -->
			<button type="button" class="btn-icon variant-filled" on:click={carouselLeft}>
				<i class="fa-solid fa-arrow-left" />
			</button>
			<!-- Full Images -->
			<div
				bind:this={elemCarousel}
				class="snap-x snap-mandatory scroll-smooth flex overflow-x-auto"
			>
				{#each movie.images as image}
					<img
						class="snap-center rounded-container-token"
						src={image}
						alt={movie.title}
						loading="lazy"
					/>
				{/each}
			</div>
			<!-- Button: Right -->
			<button type="button" class="btn-icon variant-filled" on:click={carouselRight}>
				<i class="fa-solid fa-arrow-right" />
			</button>
		</div>

		<div class="card p-4 grid grid-cols-6 gap-4">
			{#each movie.images as image, i}
				<button type="button" on:click={() => carouselThumbnail(i)}>
					<img class="rounded-container-token" src={image} alt={movie.title} loading="lazy" />
				</button>
			{/each}
		</div>
	</div>

	<section class="grid grid-cols-2 md:grid-cols-5 gap-4 md:hidden">
		{#each movie.images as image}
			<div>
				<a href={image}>
					<img
						class="rounded-container-token hover:brightness-125"
						src={image}
						alt={movie.title}
						loading="lazy"
					/>
				</a>
			</div>
		{/each}
	</section>

	<br />
	<strong class="uppercase text-l"><span style="color:#d4163c">Available</span> Showings:</strong>
	<div class="table-container">
		<table class="table table-hover">
			<thead>
				<tr>
					<th>Start Time</th>
					<th>End Time</th>
					<th>Buy Ticket</th>
				</tr>
			</thead>
			<tbody>
				{#each showings as showing}
					<tr>
						<td>{dateToUserFriendly(showing.startTime)}</td>
						<td>{dateToUserFriendly(showing.endTime)}</td>
						<td>
							<button class="btn variant-filled-primary" on:click={() => openModal(showing.number)}>
								<p>Buy <i class="fa-solid fa-money-bill" /></p>
							</button>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</div>
