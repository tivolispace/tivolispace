import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import { Document } from "mongoose";

export type UserDocument = User & Document;

@Schema()
export class User {
	id: string;

	@Prop({ required: true, unique: true })
	steamId: string;

	@Prop({ default: () => new Date() })
	created: Date;
}

export const UserSchema = SchemaFactory.createForClass(User);
