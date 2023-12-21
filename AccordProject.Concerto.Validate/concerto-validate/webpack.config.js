"use strict";

let path = require("path");
const webpack = require("webpack");

module.exports = {
  target: "node",
  entry: "./src/validate.ts",
  mode: 'production',
  plugins: [
	  new webpack.optimize.LimitChunkCountPlugin({
		maxChunks: 1,
	  })
	],
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js"],
  },
  output: {
    filename: "validate.js",
	library: 'validate',
    libraryTarget: "umd",
    path: path.resolve(__dirname, "dist"),
    //globalObject: "this",
	chunkFormat: 'module',
  },
  devtool: 'source-map'
};
