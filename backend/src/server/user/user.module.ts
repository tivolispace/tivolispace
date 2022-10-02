import { forwardRef, Module } from "@nestjs/common";
import { MongooseModule } from "@nestjs/mongoose";
import { InstanceModule } from "../instance/instance.module";
import { UserSession, UserSessionSchema } from "./user-session.schema";
import { UserSessionService } from "./user-session.service";
import { UserController } from "./user.controller";
import { UserGateway } from "./user.gateway";
import { User, UserSchema } from "./user.schema";
import { UserService } from "./user.service";

@Module({
	imports: [
		MongooseModule.forFeature([
			{ name: User.name, schema: UserSchema },
			{ name: UserSession.name, schema: UserSessionSchema },
		]),
		forwardRef(() => InstanceModule),
	],
	providers: [UserService, UserSessionService, UserGateway],
	exports: [UserService, UserSessionService],
	controllers: [UserController],
})
export class UserModule {}
