import { Module } from "@nestjs/common";
import { MongooseModule } from "@nestjs/mongoose";
import { AuthModule } from "./auth/auth.module";
import { DB_NAME, DB_URI } from "./environment";
import { UserModule } from "./user/user.module";
import { ViewModule } from "./view/view.module";

@Module({
	imports: [
		MongooseModule.forRoot(DB_URI, {
			dbName: DB_NAME,
		}),
		AuthModule,
		UserModule,
		// import view module last because it uses *
		ViewModule,
	],
	controllers: [],
	providers: [],
})
export class AppModule {}
