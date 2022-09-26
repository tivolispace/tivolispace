import {
	Box,
	Button,
	Center,
	chakra,
	Container,
	Heading,
	HStack,
	Image,
} from "@chakra-ui/react";
import { NextPage } from "next";
import { useEffect, useRef } from "react";
import introBackground from "../assets/intro-background.png";
import logoLight from "../assets/logo-light.png";
import Header from "../components/Header";
import GitHubIcon from "../icons/GitHubIcon";
import SteamIcon from "../icons/SteamIcon";

export async function getServerSideProps(context) {
	return {
		props: {
			name: "Tivoli",
		},
	};
}

const Home: NextPage = (props: { name: string }) => {
	const videoRef = useRef(null);

	useEffect(() => {
		const interval = setInterval(() => {
			const video = videoRef.current;
			if (video == null) return;
			if (video.paused) {
				video
					.play()
					.then(() => {
						clearInterval(interval);
					})
					.catch(() => {});
			} else {
				clearInterval(interval);
			}
		}, 100);
		return () => {
			clearInterval(interval);
		};
	}, []);

	const headerButtons = [
		{
			topText: "Download on",
			bottomText: "Steam",
			icon: SteamIcon,
			url: "/steam",
			bgColor: "#e91e63",
			fgColor: "#fff",
		},
		{
			topText: "Code on",
			bottomText: "GitHub",
			icon: GitHubIcon,
			url: "/github",
			bgColor: "#fff",
			fgColor: "#000",
		},
	];

	return (
		<>
			<Header onTop />
			<Box
				backgroundImage={introBackground.src}
				backgroundSize={"cover"}
				backgroundPosition={"50% 50%"}
				position={"relative"}
			>
				<Box
					position={"absolute"}
					top={0}
					right={0}
					bottom={0}
					left={0}
				>
					<chakra.video
						ref={videoRef}
						preload="auto"
						autoPlay
						muted
						loop
						playsInline
						width={"100%"}
						height={"100%"}
						background={"#000"}
						objectFit={"cover"}
					>
						<source src="/silence.mp4" type="video/mp4" />
						<source src="/silence.webm" type="video/webm" />
					</chakra.video>
				</Box>
				<Box
					position={"absolute"}
					top={0}
					right={0}
					bottom={0}
					left={0}
					backgroundColor="#1d1f21"
					opacity={0.2}
				></Box>
				<Container
					position={"relative"}
					maxW={"container.lg"}
					py={36}
					pt={48}
				>
					<Center flexDir={"column"}>
						<Image src={logoLight.src} height={32} mb={12}></Image>
						<Heading size="2xl" color="white" fontWeight={700}>
							Massive online social platform
						</Heading>
						<Heading size="xl" color="white" fontWeight={600}>
							for virtual reality and desktop
						</Heading>
						<HStack mt={12} spacing={6}>
							{headerButtons.map(
								(
									{
										topText,
										bottomText,
										icon: Icon,
										url,
										bgColor,
										fgColor,
									},
									i,
								) => (
									<Button
										key={i}
										size={"lg"}
										fontWeight={900}
										fontSize={20}
										backgroundColor={bgColor}
										_hover={{
											backgroundColor: bgColor,
										}}
										_focus={{
											backgroundColor: bgColor,
										}}
										color={fgColor}
										py={"28px"}
										px={"20px"}
										borderRadius={12}
										as={"a"}
										href={url}
									>
										<Icon
											width={8}
											height={8}
											mr={3}
											color={fgColor}
										/>
										<Center flexDir={"column"}>
											<Heading
												size={"sm"}
												fontWeight={600}
											>
												{topText}
											</Heading>
											<br />
											<Heading
												size={"md"}
												mt={-1}
												fontWeight={900}
											>
												{bottomText}
											</Heading>
										</Center>
									</Button>
								),
							)}
						</HStack>
					</Center>
				</Container>
			</Box>
		</>
	);
};

export default Home;
