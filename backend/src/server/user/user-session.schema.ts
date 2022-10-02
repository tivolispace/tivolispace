import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import * as mongoose from "mongoose";
import { Document } from "mongoose";
import { User } from "./user.schema";

export type UserSessionDocument = UserSession & Document;

export const HeartbeatTimeMs = 60 * 1000; // 1 minute

@Schema({ collection: "users.sessions" })
export class UserSession {
	id: string;

	@Prop({ type: mongoose.Schema.Types.ObjectId, ref: "User" })
	user: User;

	@Prop({ default: () => Date.now() + HeartbeatTimeMs, expires: 0 })
	expiresAt: Date;
}

export const UserSessionSchema = SchemaFactory.createForClass(UserSession);
