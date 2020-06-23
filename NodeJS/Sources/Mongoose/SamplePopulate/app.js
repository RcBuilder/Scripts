const config = {
    connStr: 'mongodb://localhost:27017/testDB'
}

// Important! MUST run the MongoDB server
const mongoose = require('mongoose');  // npm install --save mongoose

// connect to data base
(async function () {
    await mongoose.connect(config.connStr, {
        useNewUrlParser: true,
        useUnifiedTopology: true
    });
    console.log('connected to mongo');
})();

// ---

let Schema = mongoose.Schema;  // to generate Schemas

// create a Schema to represents a User
const UserSchema = new Schema({
    name: String
});
const UserModel = mongoose.model('User', UserSchema); // create a User model

// create a Schema to represents a Post
const PostSchema = new Schema({
    id: Number,
    title: { type: String, default: 'Untitled Post' },    
    createdDate: { type: Date, default: Date.now },
    user: {
        type: Schema.ObjectId,
        ref: 'User'
    }
});
const PostModel = mongoose.model('Post', PostSchema); // create a Post model

// ---

(async function () {
    try {        
        // add a user
        let user = new UserModel({ name: 'Roby' });
        await user.save();

        // add some posts
        let post1 = new PostModel({ id: 1, title: 'Post-1', user: user._id });
        let post2 = new PostModel({ id: 2, title: 'Post-2', user: user._id });
        let post3 = new PostModel({ id: 3, user: user._id });
        await post1.save();
        await post2.save();
        await post3.save();        

        // get populated posts
        let posts = await PostModel.find();
        let populated = await PostModel.populate(posts, { path: 'user' });
        console.log(populated);
    }
    catch (error) {
        console.error(error);
    };
})();

