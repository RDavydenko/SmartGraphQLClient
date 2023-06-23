using SmartGraphQLClient.Contracts;

namespace SmartGraphQLClient.GraphQLServer.Database
{
    public static class DatabaseMock
    {
        public static List<UserModel> Users { get; private set; } = new();
        public static List<RoleModel> Roles { get; private set; } = new();
        public static List<PositionModel> Positions { get; private set; } = new();

        public static void SeedDatabase()
        {
            var user1 = new UserModel
            {
                Id = 1,
                Age = 18,
                UserName = "Roman"
            };

            var user2 = new UserModel
            {
                Id = 2,
                Age = 22,
                UserName = "Michael"
            };

            var user3 = new UserModel
            {
                Id = 3,
                Age = 35,
                UserName = "Julia"
            };

            var user4 = new UserModel
            {
                Id = 4,
                Age = 12,
                UserName = "Alexander"
            };

            var user5 = new UserModel
            {
                Id = 5,
                Age = 27,
                UserName = "Elizabeth"
            };

            Users.AddRange(new[] { user1, user2, user3, user4, user5 });

            var role1 = new RoleModel
            {
                Id = 1,
                Code = RoleCode.VIEWER,
                Name = "Viewer",
                Description = "A viewer is an individual who has access to view content or participate in discussions within a particular platform or community. As a viewer, their primary role is to consume the information, media, or discussions provided by the platform or community. They can browse through various content, read articles, view images, watch videos, or listen to audio files. Viewers typically do not have the ability to create or modify content but can engage by liking, commenting, or sharing the content they find interesting."
            };

            var role2 = new RoleModel
            {
                Id = 2,
                Code = RoleCode.EDITOR,
                Name = "Editor",
                Description = "An editor is a user with elevated privileges in a platform or community who has the ability to create, modify, and curate content. Editors play a crucial role in ensuring the quality and accuracy of the information available. They can create new articles, posts, or media, as well as edit and improve existing content. Editors may have additional responsibilities such as fact-checking, proofreading, and organizing content in a structured manner. Their aim is to provide valuable and engaging content for viewers and uphold the standards set by the platform or community."
            };

            var role3 = new RoleModel
            {
                Id = 3,
                Code = RoleCode.MODERATOR,
                Name = "Moderator",
                Description = "A moderator is an individual entrusted with the responsibility of enforcing rules, guidelines, and maintaining a positive and respectful environment within a platform or community. Moderators play a crucial role in facilitating discussions, resolving conflicts, and preventing abuse or spam. They monitor user interactions, review and approve user-generated content, and may have the authority to warn or ban users who violate the community guidelines. Moderators aim to foster a safe and inclusive space for users to engage and contribute."
            };

            var role4 = new RoleModel
            {
                Id = 4,
                Code = RoleCode.ADMINISTRATOR,
                Name = "Administrator",
                Description = "An administrator is a user with the highest level of authority and control within a platform or community. Administrators have the power to manage the entire system, including user accounts, permissions, and settings. They have access to administrative tools that enable them to configure the platform, add or remove features, and oversee the activities of editors, moderators, and viewers. Administrators often make strategic decisions regarding the platform's direction, user policies, and overall community development. They bear the responsibility of ensuring the platform operates smoothly and efficiently."
            };

            Roles.AddRange(new[] { role1, role2, role3, role4 });

            user1.Roles = new[] { role1, role4 };
            user2.Roles = new[] { role1 };
            user3.Roles = new[] { role1, role2, role3, role4 };
            user4.Roles = new[] { role3, role4 };
            user5.Roles = Array.Empty<RoleModel>();

            role1.Users = new[] { user1, user2, user3 };
            role2.Users = new[] { user3 };
            role3.Users = new[] { user3, user4 };
            role4.Users = new[] { user1, user3, user4 };

            var position1 = new PositionModel
            {
                Id = 1,
                Name = "Office worker"
            };

            var position2 = new PositionModel
            {
                Id = 2,
                Name = "Chief"
            };

            Positions.AddRange(new[] { position1, position2 });

            user1.Position = user2.Position = position2;
            user3.Position = user4.Position = position1;

            position1.Users = new[] { user3, user4 };
            position2.Users = new[] { user1, user2 };
        }
    }
}
