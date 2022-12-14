import { Logger, ValidationPipe } from "@nestjs/common";
import { NestFactory } from "@nestjs/core";
import { NestExpressApplication } from "@nestjs/platform-express";
import { WsAdapter } from "@nestjs/platform-ws";
import { DocumentBuilder, SwaggerModule } from "@nestjs/swagger";
import * as cookieParser from "cookie-parser";
import { Request, Response } from "express";
import { AppModule } from "./app.module";
import { DEV, PORT } from "./environment";

function initDebugLogs(app: NestExpressApplication, logger: Logger) {
	app.use((req: Request, res: Response, next: () => void) => {
		// bodyParser.json()(req, res, () => {
		// 	bodyParser.urlencoded({
		// 		extended: true,
		// 	})(req, res, () => {
		logger.verbose(req.method + " " + req.originalUrl);
		logger.verbose("auth: " + req.headers.authorization);
		logger.verbose("body: " + JSON.stringify(req.body, null, 4));
		logger.verbose("");
		next();
		// 	});
		// });
	});
}

function initSwagger(app: NestExpressApplication) {
	const config = new DocumentBuilder()
		.setTitle("Tivoli Space")
		// .setDescription("The cats API description")
		// .setVersion("0.1")
		// .addTag("cats")
		.addBearerAuth()
		.build();

	const document = SwaggerModule.createDocument(app, config);

	SwaggerModule.setup("api", app, document);
}

async function bootstrap() {
	const app = await NestFactory.create<NestExpressApplication>(AppModule);
	const logger = new Logger("HttpServer");

	// https://expressjs.com/en/guide/behind-proxies.html
	app.set("trust proxy", 1);

	// app.enableCors({
	// 	origin: [
	// 		"null", // chrome file://
	// 		"file://",
	// 		new URL(METAVERSE_URL).origin,
	// 		new URL(FILES_URL).origin,
	// 		new URL(WORLD_URL).origin,
	// 		new URL(TEA_URL).origin,
	// 	],
	// });

	app.use(
		// 	helmet({
		// 		contentSecurityPolicy: false,
		// 	}),
		// 	// compression(),
		cookieParser(),
	);

	app.useWebSocketAdapter(new WsAdapter(app));

	app.useGlobalPipes(
		new ValidationPipe({
			// disableErrorMessages: true,
			// dismissDefaultMessages: true,
			validationError: { target: false, value: false },
			transform: true,
		}),
	);

	if (DEV) {
		initDebugLogs(app, logger);
	}
	initSwagger(app);

	// redirects
	const redirects = {
		"/steam": "https://store.steampowered.com/app/2161040",
		"/github": "https://github.com/tivolispace",
	};

	for (const [path, redirect] of Object.entries(redirects)) {
		app.use(path, (req: Request, res: Response) => {
			res.redirect(redirect);
		});
	}

	const port = parseInt(PORT);
	await app.listen(port, () => {
		logger.log("Server listening on *:" + port);
	});
}

bootstrap();
