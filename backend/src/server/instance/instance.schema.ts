import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import { Document } from "mongoose";
import { User } from "../user/user.schema";
import * as mongoose from "mongoose";
import { HeartbeatTimeMs } from "../user/user-session.schema";

export type InstanceDocument = Instance & Document;

@Schema({ collection: "instances" })
export class Instance {
	id: string;

	@Prop({ required: true, type: mongoose.Schema.Types.ObjectId, ref: "User" })
	owner: User;

	@Prop({ default: () => new Date() })
	createdAt: Date;

	@Prop({ default: () => Date.now() + HeartbeatTimeMs, expires: 0 })
	expiresAt: Date;

	@Prop({ required: true })
	connectionUri: string;
}

export const InstanceSchema = SchemaFactory.createForClass(Instance);
