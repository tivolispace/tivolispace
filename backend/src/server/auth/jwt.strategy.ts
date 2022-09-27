import { Injectable } from "@nestjs/common";
import { PassportStrategy } from "@nestjs/passport";
import { Request } from "express";
import { ExtractJwt, Strategy } from "passport-jwt";
import { JWT_SECRET } from "../environment";
import { UserService } from "../user/user.service";

export interface JwtPayload {
	id: string;
}

@Injectable()
export class JwtStrategy extends PassportStrategy(Strategy, "jwt") {
	constructor(private readonly userService: UserService) {
		super({
			jwtFromRequest: (req: Request) => {
				if (req.headers.authorization != null) {
					return ExtractJwt.fromAuthHeaderAsBearerToken()(req);
				} else if (req.cookies.auth != null) {
					try {
						const auth: { access_token: string } = JSON.parse(
							req.cookies.auth,
						);
						if (auth.access_token == null) return "";
						return auth.access_token;
					} catch (err) {
						return "";
					}
				} else {
					return "";
				}
			},
			secretOrKey: JWT_SECRET,
			ignoreExpiration: false,
		});
	}

	async validate(payload: JwtPayload) {
		const user = await this.userService.findById(payload.id);
		return user;
	}
}
