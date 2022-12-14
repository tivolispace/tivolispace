import { Prop, Schema, SchemaFactory } from "@nestjs/mongoose";
import { Document } from "mongoose";

export type UserDocument = User & Document;

@Schema({ collection: "users" })
export class User {
	id: string;

	@Prop({ required: true, unique: true })
	steamId: string;

	@Prop({ default: () => new Date() })
	createdAt: Date;

	@Prop()
	displayName: string;

	@Prop()
	profilePictureUrl: string;
}

export const UserSchema = SchemaFactory.createForClass(User);
