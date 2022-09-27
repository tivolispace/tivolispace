import { Controller, Get, Req, Res } from "@nestjs/common";
import { ApiTags } from "@nestjs/swagger";
import { Request, Response } from "express";
import { ViewService } from "./view.service";

@ApiTags("view")
@Controller("/")
export class ViewController {
	constructor(private readonly viewService: ViewService) {}

	@Get("*")
	public async static(@Req() req: Request, @Res() res: Response) {
		const server = this.viewService.getNextServer();
		const handler = server.getRequestHandler();
		handler(req, res);
	}
}
