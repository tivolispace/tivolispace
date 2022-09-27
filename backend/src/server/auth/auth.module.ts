import { Module } from "@nestjs/common";
import { JwtModule } from "@nestjs/jwt";
import { PassportModule } from "@nestjs/passport";
import { UserModule } from "../user/user.module";
import { AuthController } from "./auth.controller";
import { AuthService } from "./auth.service";
import { JwtStrategy } from "./jwt.strategy";
import { SteamStrategy } from "./steam.strategy";

@Module({
	imports: [
		PassportModule.register({
			defaultStrategy: "jwt",
		}),
		JwtModule.register({}),
		UserModule,
	],
	providers: [AuthService, SteamStrategy, JwtStrategy],
	controllers: [AuthController],
})
export class AuthModule {}
