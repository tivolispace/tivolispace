import { Injectable, OnModuleInit } from "@nestjs/common";
import { ConfigService } from "@nestjs/config";
import createServer from "next";
import { NextServer } from "next/dist/server/next";

@Injectable()
export class ViewService implements OnModuleInit {
	private server: NextServer;

	constructor(private configService: ConfigService) {}

	async onModuleInit() {
		try {
			this.server = createServer({
				dev:
					this.configService.get<string>("NODE_ENV") == "development",
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