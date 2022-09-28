import { Module } from "@nestjs/common";
import { User, UserSchema } from "./user.schema";
import { UserService } from "./user.service";
import { MongooseModule } from "@nestjs/mongoose";

@Module({
	imports: [
		MongooseModule.forFeature([{ name: User.name, schema: UserSchema }]),
	],
	providers: [UserService],
	exports: [UserService],
})
export class UserModule {}
