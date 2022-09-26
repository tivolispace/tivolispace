import { ChakraProvider } from "@chakra-ui/react";
import { extendTheme } from "@chakra-ui/react";
import "@fontsource/inter/100.css";
import "@fontsource/inter/200.css";
import "@fontsource/inter/300.css";
import "@fontsource/inter/400.css";
import "@fontsource/inter/500.css";
import "@fontsource/inter/600.css";
import "@fontsource/inter/700.css";
import "@fontsource/inter/800.css";
import "@fontsource/inter/900.css";
import "../assets/fonts/torus.css";
import Head from "next/head";

const theme = extendTheme({
	colors: {
		// material design pink
		brand: {
			50: "#fce4ec",
			100: "#f8bbd0",
			200: "#f48fb1",
			300: "#f06292",
			400: "#ec407a",
			500: "#e91e63",
			600: "#d81b60",
			700: "#c2185b",
			800: "#ad1457",
			900: "#880e4f",
			// a100: "#ff80ab",
			// a200: "#ff4081",
			// a400: "#f50057",
			// a700: "#c51162",
		},
	},
	fonts: {
		heading: `'Torus', sans-serif`,
		body: `'Inter', sans-serif`,
	},
});

function MyApp({ Component, pageProps }) {
	return (
		<ChakraProvider theme={theme}>
			<Head>
				<title>Tivoli Space</title>
				<meta name="title" content="Tivoli Space" />
				<meta
					name="description"
					content="Open source virtual worlds 🌼🌺🌹"
				/>
				<link
					rel="apple-touch-icon"
					sizes="180x180"
					href="/apple-touch-icon.png"
				/>
				<link
					rel="icon"
					type="image/png"
					sizes="32x32"
					href="/favicon-32x32.png"
				/>
				<link
					rel="icon"
					type="image/png"
					sizes="16x16"
					href="/favicon-16x16.png"
				/>
				<link rel="manifest" href="/site.webmanifest" />
				<link
					rel="mask-icon"
					href="/safari-pinned-tab.svg"
					color="#5bbad5"
				/>
				<meta name="msapplication-TileColor" content="#e91e63" />
				<meta name="theme-color" content="#ffffff" />
			</Head>
			<Component {...pageProps} />
		</ChakraProvider>
	);
}

export default MyApp;
