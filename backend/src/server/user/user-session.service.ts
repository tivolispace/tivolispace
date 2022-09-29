import { Injectable } from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Model } from "mongoose";
import { HeartbeatDto } from "./heartbeat.dto";
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

	async heartbeatUser(user: User, heartbeatDto: HeartbeatDto) {
		if (heartbeatDto.closingGame) {
			await this.userSessionModel.deleteOne({ user }).catch(() => {});
			return { id: null };
		}

		let session = await this.userSessionModel.findOne({ user });
		if (session == null) {
			session = new this.userSessionModel({ user });
		}

		session.expiresAt = new Date(Date.now() + HeartbeatTimeMs);

		session.hosting = heartbeatDto.hosting;

		await session.save();

		return { id: session.id };
	}

	async isOnline(user: User) {
		const session = await this.userSessionModel.findOne({ user });
		return session != null && session.expiresAt > new Date();
	}

	async onlineUserIds() {
		const sessions = await this.userSessionModel.find({
			expiresAt: { $gte: Date.now() },
		});
		return sessions.map(session => session.user as any as string);
	}
}
