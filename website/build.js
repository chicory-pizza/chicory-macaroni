import {promises as fs} from 'fs';
import markdownit from 'markdown-it';

async function build() {
	const html = await buildHtmlTemplate();

	await fs.rm('dist', {recursive: true, force: true});
	await fs.mkdir('dist');
	await fs.cp('public/', 'dist/', {recursive: true});
	await fs.writeFile('dist/index.htm', html);

	console.log('Done');
}

async function buildHtmlTemplate() {
	let html = await fs.readFile('template.htm', 'utf-8');
	html = html.replace('{{ readme }}', await renderMarkdownAsHtml());
	return html;
}

async function renderMarkdownAsHtml() {
	const readme = await fs.readFile('../README.md', 'utf-8');
	const md = markdownit({html: true});

	return md.render(readme);
}

build();
