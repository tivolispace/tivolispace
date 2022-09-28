import { Injectable } from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Model, ObjectId } from "mongoose";
import {
	HeartbeatTimeMs,
	UserSession,
	UserSessionDocument,
} from "./user-session.schema";
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

		session.expiresAt = new Date(Date.now() + HeartbeatTimeMs);
		await session.save();

		return { id: session.id };
	}

	async isOnline(user: User) {
		const session = await this.userSessionModel.findOne({ user });
		return session != null && session.expiresAt > new Date();
	}

	async onlineCount() {
		return await this.userSessionModel.countDocuments();
	}

	async onlineUserIds() {
		// TODO: doesnt check expiresAt
		var sessions = await this.userSessionModel.find({});
		var userIds = sessions.map(session => session.user as any as string);
		return userIds;
	}
}
