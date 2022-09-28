import { Module } from "@nestjs/common";
import { MongooseModule } from "@nestjs/mongoose";
import { AuthModule } from "./auth/auth.module";
import { DB_URI } from "./environment";
import { UserModule } from "./user/user.module";
import { ViewModule } from "./view/view.module";

@Module({
	imports: [
		MongooseModule.forRoot(DB_URI),
		AuthModule,
		UserModule,
		// import view module last because it uses *
		ViewModule,
	],
	controllers: [],
	providers: [],
})
export class AppModule {}
