import { Injectable, UnauthorizedException } from "@nestjs/common";
import { AuthGuard } from "@nestjs/passport";

@Injectable()
export class TivoliAuthGuard extends AuthGuard("jwt") {
	handleRequest(error, user, info, context) {
		if (error) throw new UnauthorizedException();
		if (user.id == null) throw new UnauthorizedException();

		// can check for email verification here

		// check for banned here too maybe

		return user;
	}
}
