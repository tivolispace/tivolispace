import { Injectable, OnModuleInit } from "@nestjs/common";
import createServer from "next";
import { NextServer } from "next/dist/server/next";
import { DEV } from "../environment";

@Injectable()
export class ViewService implements OnModuleInit {
	private server: NextServer;

	constructor() {}

	async onModuleInit() {
		try {
			this.server = createServer({
				dev: !!DEV,
				dir: "./src/client",
			});
			await this.server.prepare();
		} catch (error) {
			console.error(error);
		}
	}

	getNextServer(): NextServer {
		return this.server;
	}
}
