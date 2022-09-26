import {
	Avatar,
	Box,
	Button,
	Center,
	chakra,
	Flex,
	Heading,
	HStack,
	Image,
	Text,
} from "@chakra-ui/react";
import { MdBookmarks, MdPeopleAlt } from "react-icons/md";
import logoLightNoPink from "../assets/logo-light-no-pink.png";

export default function Header(props: { onTop?: boolean }) {
	const onTop = props.onTop;

	const links = [
		{ name: "Docs", icon: <MdBookmarks size={22} /> },
		{ name: "About us", icon: <MdPeopleAlt size={22} /> },
	];

	return (
		<Flex
			backgroundColor={onTop ? "transparent" : "brand.500"}
			position={onTop ? "absolute" : null}
			top={0}
			left={0}
			right={0}
			zIndex={999}
			px={4}
			py={2}
			flexDir={"row"}
			alignItems={"center"}
			justifyContent={"center"}
		>
			<Image src={logoLightNoPink.src} height={8} />
			<HStack ml={8} spacing={8}>
				{links.map(({ name, icon }, i) => (
					<Button
						key={i}
						variant={"link"}
						color={"white"}
						_hover={{ textDecoration: "none" }}
						_focus={{ color: "white" }}
						leftIcon={icon}
						fontWeight={600}
						as="a"
					>
						<Heading size={"sm"} fontWeight={600}>
							{name}
						</Heading>
					</Button>
				))}
			</HStack>
			<Box flexGrow={1}></Box>
			<Button
				// backgroundColor={loginBackground}
				// _hover={{ bg: loginBackground }}
				// _focus={{ bg: loginBackground }}
				// pl={"20px"}
				// pr={"4px"}
				// py={"24px"}
				variant={"unstyled"}
				borderRadius={999}
				display={"flex"}
			>
				<Center flexDir={"column"} alignItems="flex-end">
					<Box
						display={"flex"}
						flexDir={"row"}
						alignItems="flex-start"
						mt={0}
					>
						<Text
							color="white"
							fontSize={12}
							fontWeight={600}
							mr={1}
							mt={"3px"}
						></Text>
						<Heading size={"sm"} color="white">
							<chakra.span fontWeight={600} fontSize={14}>
								Signed in as
							</chakra.span>{" "}
							Maki
						</Heading>
					</Box>
					<Text
						color="white"
						fontSize={12}
						fontWeight={400}
						opacity={0.8}
						mt={-0.5}
					>
						email@example.com
					</Text>
				</Center>
				<Avatar
					width={10}
					height={10}
					ml={2}
					// borders="solid 2px white"
					src="https://cdn.discordapp.com/avatars/72139729285427200/030849ff09ed741ce2d3c7ac8b3cb426.webp?size=128"
				/>
			</Button>
		</Flex>
	);
}
