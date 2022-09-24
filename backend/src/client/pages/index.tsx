import React from "react";
import { NextPage } from "next";
import { Heading } from "@chakra-ui/react";
import TorusBold from "../components/TorusBold";

export async function getServerSideProps(context) {
	return {
		props: {
			name: "Tivoli",
		},
	};
}

const Home: NextPage = (props: { name: string }) => {
	return (
		<>
			<Heading fontSize={64}>Yay, it's {props.name}</Heading>
			<TorusBold size={64}>Yay, it's {props.name}</TorusBold>
			<TorusBold size={32}>Logged in as Maki</TorusBold>
			<TorusBold size={16}>Size 16 text probably too small!</TorusBold>
			<TorusBold size={64}>
				This is really long it should look wrong until it gets fixed
				which hopefully it already is!
			</TorusBold>
		</>
	);
};

export default Home;
