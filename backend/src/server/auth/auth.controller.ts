import { Body, Controller, Get, Post, Req, UseGuards } from "@nestjs/common";
import { AuthGuard } from "@nestjs/passport";
import { ApiBearerAuth, ApiTags } from "@nestjs/swagger";
import { Request } from "express";
import { TivoliAuthGuard } from "./auth.guard";
import { AuthService } from "./auth.service";
import { SteamTicketDto } from "./steam-ticket.dto";
import { SteamProfile } from "./steam.strategy";
import { CurrentUser } from "./user.decorator";

@ApiTags("auth")
@Controller("api/auth")
export class AuthController {
	constructor(private readonly authService: AuthService) {}

	@Get("steam")
	@UseGuards(AuthGuard("steam"))
	async steam() {
		// guard redirects
	}

	@Get("steam/callback")
	@UseGuards(AuthGuard("steam"))
	async steamCallback(@Req() req: Request) {
		const user = req.user as SteamProfile;
		return this.authService.login(user.id);
	}

	@Post("steam-ticket")
	async steamTicket(@Body() steamTicketDto: SteamTicketDto) {
		return this.authService.loginSteamTicket(steamTicketDto.ticket);
	}

	@ApiBearerAuth()
	@Get("test")
	@UseGuards(TivoliAuthGuard)
	async test(@CurrentUser() user) {
		return { user };
	}
}
