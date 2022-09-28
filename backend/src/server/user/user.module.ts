import { Module } from "@nestjs/common";
import { MongooseModule } from "@nestjs/mongoose";
import { UserSession, UserSessionSchema } from "./user-session.schema";
import { User, UserSchema } from "./user.schema";
import { UserService } from "./user.service";
import { UserSessionService } from "./user-session.service";
import { UserController } from "./user.controller";

@Module({
	imports: [
		MongooseModule.forFeature([
			{ name: User.name, schema: UserSchema },
			{ name: UserSession.name, schema: UserSessionSchema },
		]),
	],
	providers: [UserService, UserSessionService],
	exports: [UserService],
	controllers: [UserController],
})
export class UserModule {}
