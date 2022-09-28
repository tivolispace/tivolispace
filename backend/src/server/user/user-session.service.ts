import { Injectable } from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Model } from "mongoose";
import { UserSession, UserSessionDocument } from "./user-session.schema";
import { User } from "./user.schema";

@Injectable()
export class UserSessionService {
	constructor(
		@InjectModel(UserSession.name)
		private readonly userSessionModel: Model<UserSessionDocument>,
	) {}

	async heartbeatUser(user: User) {
		let session = await this.userSessionModel.findOne({ user });
		if (session == null) {
			session = new this.userSessionModel({ user });
		}

		session.expiresAt = new Date(Date.now() + 60);
		await session.save();

		return session;
	}

	async isOnline(user: User) {
		const session = await this.userSessionModel.findOne({ user });
		return session != null && session.expiresAt > new Date();
	}
}