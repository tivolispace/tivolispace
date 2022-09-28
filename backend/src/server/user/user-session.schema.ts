import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import * as mongoose from "mongoose";
import { Document } from "mongoose";
import { User } from "./user.schema";

export type UserSessionDocument = UserSession & Document;

@Schema({ collection: "users.sessions" })
export class UserSession {
	id: string;

	@Prop({ type: mongoose.Schema.Types.ObjectId, ref: "User" })
	user: User;

	@Prop({ default: () => Date.now() + 60, expires: 0 })
	expiresAt: Date;
}

export const UserSessionSchema = SchemaFactory.createForClass(UserSession);
