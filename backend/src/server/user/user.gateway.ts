import {
	ConnectedSocket,
	MessageBody,
	OnGatewayConnection,
	OnGatewayDisconnect,
	SubscribeMessage,
	WebSocketGateway,
	WebSocketServer,
} from "@nestjs/websockets";
import * as ws from "ws";
import { UserService } from "./user.service";

// look into doing this later, will be a little difficult when we scale.
// we'll have to hold ref to socket inside user session document

@WebSocketGateway()
export class UserGateway implements OnGatewayConnection, OnGatewayDisconnect {
	constructor(private readonly userService: UserService) {}

	@WebSocketServer() server: ws.WebSocketServer;

	async handleConnection(@ConnectedSocket() client: ws.WebSocket) {}

	async handleDisconnect(@ConnectedSocket() client: ws.WebSocket) {}

	@SubscribeMessage("auth")
	async onAuth(
		@ConnectedSocket() client: ws.WebSocket,
		@MessageBody() data: any,
	) {
		if (data.accessToken) {
		}
	}
}
